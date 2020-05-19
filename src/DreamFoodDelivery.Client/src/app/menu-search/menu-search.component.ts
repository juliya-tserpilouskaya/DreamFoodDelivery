import { Component, OnInit } from '@angular/core';

// import service and models
import { MenuService, DishView, BasketService, DishToBasketAdd, TagService, TagView, TagToAdd, SearchService } from '../app-services/nswag.generated.services';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, FormArray, FormControl } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { ManageMenuService } from '../app-services/manage-menu.service';

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

  page = 2;
  pageSize = 9;
  parameter: string;

  searchForm: FormGroup;
  addForm: FormGroup;

  constructor(
    private menuService: MenuService,
    private basketService: BasketService,
    private searchService: SearchService,
    private authService: AuthService,
    private manageMenuService: ManageMenuService,
    public router: Router,
    public fb: FormBuilder
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
      .then(data => this.dishes = data)
      .catch(msg => console.log(msg));
    this.searchService.getAllTags().subscribe(data => {this.tags = data; },
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

  onAddToBasket(id: string) {
    this.addForm.value.dishId = id;
    this.basketService.addDish(this.addForm.value).subscribe(data => {
      this.addForm.reset();
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

  onSearchButtonClicked() {
    console.log(this.searchForm.value);
    this.searchService.getAllDishesByRequest(this.searchForm.value).subscribe(data => {this.dishes = data;
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

  onChange(name: string, isChecked: boolean) {
    const tagsNames = (this.searchForm.controls.tagsNames as FormArray);
    if (isChecked) {
      tagsNames.push(new FormControl(name));
    } else {
      const index = tagsNames.controls.findIndex(x => x.value === name);
      tagsNames.removeAt(index);
    }
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }
}
