import { Injectable } from '@angular/core';
import { DishView, MenuService, DishService } from './nswag.generated.services';

@Injectable({
  providedIn: 'root'
})
export class ManageMenuService {
  dishes: DishView[] = [];

  constructor(
    private menuService: MenuService,
    private dishService: DishService
  ) { }

  getAllDishes(): Promise<DishView[]> {
    // if (this.dishes.length) {
    //   return new Promise(resolve => resolve(this.dishes));
    // }
    return new Promise((resolve, reject)  => {
      this.menuService.getAll().subscribe({
        next(data) {
          this.dishes = data;
          resolve(data);
        },
        error(msg) {
          console.log('Error Getting Location in ManageMenuService: ', msg);
          reject(msg);
        },
      });
    }
    );
  }

  removeAll(): Promise<DishView[]> {
    return new Promise((resolve, reject)  => {
      this.dishService.removeAll().subscribe({
        next(data) {
          this.dishes = null;
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
}
