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
      .catch(msg => {console.log(status);
                     this.orders = null; });
    this.manageOrderService.getStatuses()
      .then(data => this.orderStatuses = data)
      .catch(msg => console.log(status));
  }

  statusUpdate(id: string): void {
    this.statusUpdateForm.value.id = id;
    this.statusUpdateForm.value.statusIndex = +this.statusUpdateForm.value.statusIndex;
    // console.log(this.statusUpdateForm.value);
    this.orderService.updateStatus(this.statusUpdateForm.value).subscribe(data => {
      window.location.reload();
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