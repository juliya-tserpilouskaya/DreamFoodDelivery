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

  constructor(
    private manageOrderService: ManageOrderService,
  ) { }

  ngOnInit(): void {
    this.manageOrderService.getStatuses()
      .then(data => this.orderStatuses = data)
      .catch(msg => console.log(status));
  }
}
