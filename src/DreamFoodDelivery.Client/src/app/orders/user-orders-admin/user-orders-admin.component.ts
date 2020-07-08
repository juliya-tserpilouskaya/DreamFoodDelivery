import { Component, OnInit } from '@angular/core';
import { OrderView, OrderService } from 'src/app/app-services/nswag.generated.services';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-user-orders-admin',
  templateUrl: './user-orders-admin.component.html',
  styleUrls: ['./user-orders-admin.component.scss']
})
export class UserOrdersAdminComponent implements OnInit {
  idFromURL = '';
  message: string = null;
  orders: OrderView[] = [];

  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute,
    public router: Router,
    private location: Location,
  ) {
    route.params.subscribe(params => this.idFromURL = params.id);
  }

  ngOnInit(): void {
    this.orderService.getByUserIdAdmin(this.idFromURL).subscribe(data => {this.orders = data;
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  500) {
        this.message = error.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
  });
  }

  goBack(): void {
    this.location.back();
  }

  removeById(id: string): void {
    this.orderService.removeById(id).subscribe(data => {
      const indexToDelete = this.orders.findIndex((mark: OrderView) => mark.id === id);
      this.orders.splice(indexToDelete, 1);
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  500) {
        this.message = error.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }

  removeAll(): void {
    this.orderService.removeAllByUserId(this.idFromURL).subscribe(result => {
      this.orders = null;
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  404) {
        this.message = 'Elements are not found.';
      }
      else if (error.status ===  500) {
        this.message = error.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }
}
