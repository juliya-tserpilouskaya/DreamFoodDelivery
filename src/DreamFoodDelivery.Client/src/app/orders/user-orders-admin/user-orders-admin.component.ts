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
      if (error.status === 500){
        this.router.navigate(['/error/500']);
       }
       else if (error.status === 404) {
        this.router.navigate(['/error/404']);
       }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
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
      if (error.status === 500){
        this.router.navigate(['/error/500']);
       }
       else if (error.status === 404) {
        this.router.navigate(['/error/404']);
       }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
    });
  }

  removeAll(): void {
    this.orderService.removeAllByUserId(this.idFromURL).subscribe(result => {
      this.orders = null;
    },
    error => {
      if (error.status === 500){
        this.router.navigate(['/error/500']);
       }
       else if (error.status === 404) {
        this.router.navigate(['/error/404']);
       }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
    });
  }
}
