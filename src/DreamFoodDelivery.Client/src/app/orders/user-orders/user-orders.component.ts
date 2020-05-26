import { Component, OnInit } from '@angular/core';
import { OrderView, OrderService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-orders',
  templateUrl: './user-orders.component.html',
  styleUrls: ['./user-orders.component.scss']
})
export class UserOrdersComponent implements OnInit {
  orders: OrderView[] = [];

  constructor(
    private orderService: OrderService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.orderService.getByUserId().subscribe(data => {this.orders = data;
    },
    error => {
      // if (error.status === 500){
      //   this.router.navigate(['/error/500']);
      //  }
      //  else if (error.status === 404) {
      //   this.router.navigate(['/error/404']);
      //  }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
      });
  }
}
