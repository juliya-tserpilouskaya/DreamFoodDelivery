import { Component, OnInit } from '@angular/core';
import { OrderStatus, OrderService } from 'src/app/app-services/nswag.generated.services';
import { ManageOrderService } from 'src/app/app-services/manage-order.service';

@Component({
  selector: 'app-employee-nav-bar',
  templateUrl: './employee-nav-bar.component.html',
  styleUrls: ['./employee-nav-bar.component.scss']
})
export class EmployeeNavBarComponent implements OnInit {
  orderStatuses: OrderStatus[] = [];
  message: string = null;

  constructor(
    private manageOrderService: ManageOrderService,
  ) { }

  ngOnInit(): void {
    this.manageOrderService.getStatuses()
      .then(data => this.orderStatuses = data)
      .catch(msg => {
        if (msg.status ===  204) {
          this.message = msg.response;
        }
        else if (msg.status ===  400) {
          this.message = 'Error 400: ' + msg.response;
        }
        else if (msg.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      });
  }
}
