import { Injectable } from '@angular/core';
import { OrderService, OrderStatus, OrderView } from './nswag.generated.services';

@Injectable({
  providedIn: 'root'
})
export class ManageOrderService {
  orderStatuses: OrderStatus[] = [];
  ordersInStatus: OrderView[] = [];
  ordersAll: OrderView[] = [];

  constructor(
    private orderService: OrderService,
  ) { }

  getStatuses(): Promise<OrderStatus[]> {
    if (this.orderStatuses.length) {
      return new Promise(resolve => resolve(this.orderStatuses));
    }
    return new Promise((resolve, reject)  => {
      this.orderService.getStatuses().subscribe({
        next(data) {
          this.orderStatuses = data;
          resolve(data);
        },
        error(msg) {
          console.log('Error Getting Location: ', msg);
          reject(msg);
        },
      });
    }
    );
  }

  getOrdersByStatus(statusName: string): Promise<OrderView[]> {
    // if (this.orders.length) {
    //   return new Promise(resolve => resolve(this.orders));
    // }
    return new Promise((resolve, reject)  => {
      this.orderService.getOrdersInStatus(statusName).subscribe({
        next(data) {
          this.ordersInStatus = data;
          resolve(data);
        },
        error(msg) {
          console.log('Error Getting Location in Promise: ', msg);
          console.log(msg.status);
          console.log(msg._responseText);

          console.log(msg.result404);
          reject(msg);
        },
      });
    }
    );
  }

  getAllOrders(): Promise<OrderView[]> {
    // if (this.ordersAll.length) {
    //   return new Promise(resolve => resolve(this.ordersAll));
    // }
    return new Promise((resolve, reject)  => {
      this.orderService.getAll().subscribe({
        next(data) {
          this.ordersAll = data;
          resolve(data);
        },
        error(msg) {
          console.log('Error Getting Location in Promise: ', msg);
          console.log(msg.status); // +
          console.log(msg._responseText);
          reject(msg);
        },
      });
    }
    );
  }

  removeAll(): Promise<OrderView[]> {
    return new Promise((resolve, reject)  => {
      this.orderService.removeAll().subscribe({
        next(data) {
          this.ordersAll = null;
          resolve(null);
        },
        error(msg) {
          console.log('Error Getting Location in ManageMenuService: ', msg);
          reject(msg);
        },
      });
    }
    );
  }



  // updateOrderStatus(id: string, newStatus: string): OrderView[] {
  //   const order = this.orders[id];
  //   order. // изменить в переменной статус.
  //   // Вернуть его обратно в массив.
  // }


}
