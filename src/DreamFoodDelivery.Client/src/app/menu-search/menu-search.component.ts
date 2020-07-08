import { Component, OnInit } from '@angular/core';

// import service and models
import { MenuService, DishView, BasketService, DishToBasketAdd, TagView, TagService } from '../app-services/nswag.generated.services';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { ManageMenuService } from '../app-services/manage-menu.service';
import { Inject } from '@angular/core';
import { ImageModifiedService } from '../app-services/image.services';

export class Cartoon {
  id: number;
  name: string;
}

@Component({
  selector: 'app-menu-search',
  templateUrl: './menu-search.component.html',
  styleUrls: ['./menu-search.component.scss']
})
export class MenuSearchComponent implements OnInit {

  dishes: DishView[] = [];
  tags: TagView[] = [];
  dishInfoToAdd: DishToBasketAdd;

  message: string = null;

  page = 2;
  pageSize = 9;
  parameter: string;

  searchForm: FormGroup;
  addForm: FormGroup;

  mainImages: {[id: string]: string} = { };

  constructor(
    private menuService: MenuService,
    private tagService: TagService,
    private basketService: BasketService,
    private authService: AuthService,
    private manageMenuService: ManageMenuService,
    private imageService: ImageModifiedService,
    public router: Router,
    public fb: FormBuilder,
    ) {
    this.addForm = fb.group({quantity: [''],
                             dishId: ['']});

    this.searchForm = fb.group({tagsNames: this.fb.array([]),
                                request: null,
                                onSale: false,
                                lowerPrice: 0,
                                upperPrice: 0});
    }

  ngOnInit(): void {
    // this.menuService.getAll().subscribe(data => {this.dishes = data; });
    this.manageMenuService.getAllDishes()
      .then(data => {this.dishes = data;
                     if (this.dishes) {
                      for (const dish of this.dishes){
                        this.getImages(dish.id);
                      }
                     }
                    })
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
        this.dishes = []; });

    this.tagService.getAllTags().subscribe(data => {this.tags = data; },
      error => {
        if (error.status ===  206) {
          this.tags = [];
        }
        else if (error.status ===  500) {
          this.message = error.message;
          this.router.navigate(['/error/500', {msg: this.message}]);
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
        this.tags = [];
    });
  }

  onAddToBasket(id: string) {
    this.addForm.value.dishId = id;
    this.basketService.addDish(this.addForm.value).subscribe(data => {
    this.addForm.reset();
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  500) {
        this.message = error.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }

  onSearchButtonClicked() {
    console.log(this.searchForm.value);
    this.menuService.getAllDishesByRequest(this.searchForm.value).subscribe(data => {this.dishes = data;
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  500) {
        this.message = error.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
      this.dishes = [];
      });
  }

  onChange(name: string, isChecked: boolean) {
    const tagsNames = (this.searchForm.controls.tagsNames as FormArray);
    if (isChecked) {
      tagsNames.push(new FormControl(name));
    } else {
      const index = tagsNames.controls.findIndex(x => x.value === name);
      tagsNames.removeAt(index);
    }
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
