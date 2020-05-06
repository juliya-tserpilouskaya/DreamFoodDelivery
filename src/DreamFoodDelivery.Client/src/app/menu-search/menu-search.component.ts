import { Component, OnInit } from '@angular/core';

// import service and models
import { MenuService, DishView, BasketService, DishToAdd, DishToBasketAdd } from '../app-services/nswag.generated.services';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-menu-search',
  templateUrl: './menu-search.component.html',
  styleUrls: ['./menu-search.component.scss']
})
export class MenuSearchComponent implements OnInit {

  dishes: DishView[] = [];
  page = 2;
  pageSize = 10;
  parameter: string;
  dishInfoToAdd: DishToBasketAdd;
  addForm: FormGroup;

  constructor(
    private menuService: MenuService,
    private basketService: BasketService,
    public route: ActivatedRoute,
    public fb: FormBuilder
    ) {
    route.params.subscribe(params => this.parameter = params.query);
    this.addForm = fb.group({Quantity: ['']});
    }


  ngOnInit(): void {
    this.menuService.getAll().subscribe(data => {this.dishes = data;
                                                 console.log(data);
                                                 console.log(this.dishes);
                                                });

  }

  onAddToBasket(id: string) {
    const data = this.addForm.value;
    this.dishInfoToAdd.dishId = id;
    this.dishInfoToAdd.quantity = data;
    this.basketService.addDish(this.dishInfoToAdd).subscribe();
  }

}
