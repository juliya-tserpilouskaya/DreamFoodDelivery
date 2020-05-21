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
      .catch(msg => console.log(msg));
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
      this.manageOrderService.removeAll().then(data => this.orders = data)
      .catch(msg => {console.log(msg); });
    // this.orderService.removeAll().subscribe(data => {window.location.reload();
    // });
  }
}
