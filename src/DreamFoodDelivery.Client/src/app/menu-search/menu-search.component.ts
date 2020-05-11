import { Component, OnInit } from '@angular/core';

// import service and models
import { MenuService, DishView, BasketService, DishToBasketAdd, TagService, TagView, TagToAdd, SearchService } from '../app-services/nswag.generated.services';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-menu-search',
  templateUrl: './menu-search.component.html',
  styleUrls: ['./menu-search.component.scss']
})
export class MenuSearchComponent implements OnInit {

  dishes: DishView[] = [];
  dishesForFilter: DishView[] = [];
  tags: TagView[] = [];
  filterTag: TagToAdd;
  page = 2;
  pageSize = 9;
  parameter: string;
  dishInfoToAdd: DishToBasketAdd;
  addForm: FormGroup;

  constructor(
    private menuService: MenuService,
    private basketService: BasketService,
    private searchService: SearchService,
    private authService: AuthService,
    public route: ActivatedRoute,
    public fb: FormBuilder
    ) {
    route.params.subscribe(params => this.parameter = params.query);
    this.addForm = fb.group({quantity: [''], dishId: ['']});
    }

  ngOnInit(): void {
    this.menuService.getAll().subscribe(data => {this.dishes = data,
                                                this.dishesForFilter = data;
                                                });
    this.searchService.getAllTags().subscribe(data => this.tags = data);
  }

  onAddToBasket(id: string) {
    this.addForm.value.dishId = id;
    this.basketService.addDish(this.addForm.value).subscribe();
  }

  get isAuthenticated(): boolean {
    return this.authService.isLoggedIn;
  }

}
