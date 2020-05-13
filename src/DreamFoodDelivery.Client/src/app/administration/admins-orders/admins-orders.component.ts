import { Component, OnInit } from '@angular/core';
import { OrderView, OrderService } from 'src/app/app-services/nswag.generated.services';

@Component({
  selector: 'app-admins-orders',
  templateUrl: './admins-orders.component.html',
  styleUrls: ['./admins-orders.component.scss']
})
export class AdminsOrdersComponent implements OnInit {
  orders: OrderView[] = [];

  constructor(
    private orderService: OrderService,
  ) { }

  ngOnInit(): void {
    this.orderService.getAll().subscribe(data => {this.orders = data;
    });
  }

  removeById(id: string): void {
    this.orderService.removeById(id).subscribe(data => {
      const indexToDelete = this.orders.findIndex((mark: OrderView) => mark.id === id);
      this.orders.splice(indexToDelete, 1);
    });
  }

  removeAll(): void {
    this.orderService.removeAll().subscribe(data => {window.location.reload();
    });
  }
}
