import { Component, OnInit } from '@angular/core';
import { OrderView, OrderService } from 'src/app/app-services/nswag.generated.services';
import { ManageOrderService } from 'src/app/app-services/manage-order.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admins-orders',
  templateUrl: './admins-orders.component.html',
  styleUrls: ['./admins-orders.component.scss']
})
export class AdminsOrdersComponent implements OnInit {
  orders: OrderView[] = [];
  message: string = null;

  page = 2;
  pageSize = 10;

  constructor(
    private orderService: OrderService,
    private manageOrderService: ManageOrderService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    // this.orderService.getAll().subscribe(data => {this.orders = data;
    // });
    this.manageOrderService.getAllOrders()
      .then(data => this.orders = data)
      .catch(msg => {
        console.log(msg.status);
        if (msg.status ===  204) {
          this.message = 'Now empty.';
          this.orders = [];
        }
        else if (msg.status ===  403) {
          this.message = 'You are not authorized!';
        }
        else if (msg.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
    });
  }

  removeById(id: string): void {
    this.orderService.removeById(id).subscribe(data => { this.ngOnInit();
      // const indexToDelete = this.orders.findIndex((mark: OrderView) => mark.id === id);
      // this.orders.splice(indexToDelete, 1);
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  404) {
        this.message = 'Element not found.';
      }
      else if (error.status ===  500) {
        this.message = 'Error 500: Internal Server Error!';
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }

  removeAll(): void {
      this.manageOrderService.removeAll().then(data => this.orders = data)
      .catch(msg => {
        if (msg.status ===  400) {
          this.message = 'Error 400: ' + msg.response;
        }
        else if (msg.status ===  403) {
          this.message = 'You are not authorized!';
        }
        else if (msg.status ===  404) {
          this.message = 'Element not found.';
        }
        else if (msg.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      });
    // this.orderService.removeAll().subscribe(data => {window.location.reload();
    // });
  }
}
