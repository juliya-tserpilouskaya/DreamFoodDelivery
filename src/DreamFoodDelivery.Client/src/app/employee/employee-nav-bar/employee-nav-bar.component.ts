import { Component, OnInit } from '@angular/core';
import { OrderStatus, OrderService } from 'src/app/app-services/nswag.generated.services';
import { ManageOrderService } from 'src/app/app-services/manage-order.service';
import { Router } from '@angular/router';

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
    public router: Router
  ) { }

  ngOnInit(): void {
    this.manageOrderService.getStatuses()
      .then(data => this.orderStatuses = data)
      .catch(msg => {
        if (msg.status ===  206) {
          this.message = msg.detail;
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
}
