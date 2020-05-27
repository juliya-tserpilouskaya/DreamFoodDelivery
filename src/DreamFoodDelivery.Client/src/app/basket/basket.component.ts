import { Component, OnInit } from '@angular/core';
import { BasketView, BasketService, DishView, OrderService } from '../app-services/nswag.generated.services';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Location } from '@angular/common';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { ImageModifiedService } from '../app-services/image.services';

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
  message: string = null;
  updateForm: FormGroup;

  mainImages: {[id: string]: string} = { };

  constructor(
    private basketService: BasketService,
    private authService: AuthService,
    private orderService: OrderService,
    private imageService: ImageModifiedService,
    private location: Location,
    public router: Router,
    public fb: FormBuilder
    ) {
    this.updateForm = fb.group({quantity: [''], dishId: ['']});
    }

  ngOnInit(): void {
    this.basketService.getAll().subscribe(data => {this.basket = data;
                                                   this.dishes = data.dishes;
                                                   if (this.dishes) {
                                                    for (const dish of this.dishes){
                                                      this.getImages(dish.id);
                                                    }
                                                   }
                                                  },
                                                  error => {
                                                    if (error.status ===  204) {
                                                      this.message = 'Now empty.';
                                                    }
                                                    else if (error.status ===  400) {
                                                      this.message = 'Error 400: ' + error.response;
                                                    }
                                                    else if (error.status ===  500) {
                                                      this.message = 'Error 500: Internal Server Error!';
                                                    }
                                                    else{
                                                      this.message = 'Something was wrong. Please, contact with us.';
                                                    }
                                                  });
  }

  onBasketUpdate(id: string): void {
    this.updateForm.value.dishId = id;
    this.basketService.addDish(this.updateForm.value).subscribe(data => {this.ngOnInit(); },
      error => {
        if (error.status ===  400) {
          this.message = 'Error 400: ' + error.response;
        }
        else if (error.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      });
  }

  removeDish(id: string): void {
    this.basketService.removeById(id).subscribe(data => {
      // const indexToDelete = this.dishes.findIndex((mark: DishView) => mark.id === id);
      // this.dishes.splice(indexToDelete, 1);
      this.ngOnInit();
    },
    error => {
if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  500) {
        this.message = 'Error 500: Internal Server Error!';
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }

  removeAll(): void {
    this.basketService.removeAll().subscribe(data => {
      this.ngOnInit();
    },
    error => {
      if (error.status ===  204) {
        this.message = 'Now empty.';
      }
      else if (error.status ===  500) {
        this.message = 'Error 500: Internal Server Error!';
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
    // this.ngOnInit();
    // window.location.reload();
  }

  goBack(): void {
    this.location.back();
  }

  getImages(id: string){
    this.imageService.getImageNamesList(id)
    .subscribe(allImages => {
      if (allImages != null && allImages.length > 0) {
        this.imageService.getImage(id, allImages[0])
        .subscribe(data => {
        this.mainImages[id] = data;
        });
      }
    });
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }

}
