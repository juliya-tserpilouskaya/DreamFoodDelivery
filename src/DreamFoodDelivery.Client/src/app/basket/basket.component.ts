import { Component, OnInit } from '@angular/core';
import { BasketView, BasketService, DishView, OrderService } from '../app-services/nswag.generated.services';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent implements OnInit {

  basket: BasketView;
  dishes: DishView[] = [];
  page = 2;
  pageSize = 10;
  updateForm: FormGroup;

  constructor(
    private basketService: BasketService,
    private authService: AuthService,
    private orderService: OrderService,
    public fb: FormBuilder
    ) {
    this.updateForm = fb.group({quantity: [''], dishId: ['']});
    }

  ngOnInit(): void {
    this.basketService.getAll().subscribe(data => {this.basket = data;
                                                   this.dishes = data.dishes;
                                                  });
  }
  onBasketUpdate(id: string): void {
    this.updateForm.value.dishId = id;
    this.basketService.addDish(this.updateForm.value).subscribe();
  }

  removeDish(id: string): void {
    this.basketService.removeById(id).subscribe(data => {
      const indexToDelete = this.dishes.findIndex((mark: DishView) => mark.id === id);
      this.dishes.splice(indexToDelete, 1);
    });
  }

  removeAll(): void {
    this.basketService.removeAll().subscribe(data => {
      window.location.reload();
    });
    // this.ngOnInit();
    // window.location.reload();
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }

}
