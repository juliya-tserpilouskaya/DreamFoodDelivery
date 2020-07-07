import { Injectable } from '@angular/core';
import { DishView, MenuService, DishService } from './nswag.generated.services';
import { AuthService } from '../auth/auth.service';
import * as jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class ManageMenuService {
  dishes: DishView[] = [];

  constructor(
    private menuService: MenuService,
    private dishService: DishService,
    private authService: AuthService,
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
          reject(msg);
        },
      });
    }
    );
  }

  isAdmin(): boolean {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token)
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedToken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }
}
