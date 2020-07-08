import { Component, OnInit } from '@angular/core';
import { OrderService, OrderView, OrderStatus, OrderToStatusUpdate } from 'src/app/app-services/nswag.generated.services';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ManageOrderService } from 'src/app/app-services/manage-order.service';

@Component({
  selector: 'app-employee-orders',
  templateUrl: './employee-orders.component.html',
  styleUrls: ['./employee-orders.component.scss']
})
export class EmployeeOrdersComponent implements OnInit {
  orders: OrderView[] = [];
  orderStatuses: OrderStatus[] = [];

  name: string;
  message: string = null;
  statusUpdateForm: FormGroup;

  constructor(
    private orderService: OrderService,
    private manageOrderService: ManageOrderService,
    public router: Router,
    route: ActivatedRoute,
    public fb: FormBuilder
  ) {
    route.params.subscribe(params => {this.name = params.statusName; this.ngOnInit(); });
    this.statusUpdateForm = fb.group({id: [''],
                                      statusIndex: null});
  }

  ngOnInit(): void {
    // this.orderService.getOrdersInStatus(this.name).subscribe(data => {this.orders = data;
    // },
    // error => {console.log(error); // TODO: закончить обработку всяких положений и настроить роутинг с сообщениями
    //           console.log(error.status);
    //           console.log(error._responseText);
    //           console.log(error._headers);
    // });
    this.manageOrderService.getOrdersByStatus(this.name)
      .then(data => this.orders = data)
      .catch(msg => {
        if (msg.status ===  403) {
          this.message = 'You are not authorized!';
        }
        else if (msg.status ===  404) {
          this.message = 'Elements are not found.';
        }
        else if (msg.status ===  500) {
          this.message = msg.message;
          this.router.navigate(['/error/500', {msg: this.message}]);
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
        this.orders = null; });
    this.manageOrderService.getStatuses()
      .then(data => this.orderStatuses = data)
      .catch(msg => {
        if (msg.status ===  206) {
          this.message = msg.detail;
        }
        else if (msg.status ===  400) {
          this.message = 'Error 400: ' + msg.response;
        }
        else if (msg.status ===  500) {
          this.message = msg.message;
          this.router.navigate(['/error/500', {msg: this.message}]);
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      });
  }

  statusUpdate(id: string): void {
    this.statusUpdateForm.value.id = id;
    this.statusUpdateForm.value.statusIndex = +this.statusUpdateForm.value.statusIndex;
    // console.log(this.statusUpdateForm.value);
    this.orderService.updateStatus(this.statusUpdateForm.value).subscribe(data => {
      this.ngOnInit();
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
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
}
