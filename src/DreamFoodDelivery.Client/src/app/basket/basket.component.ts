import { Component, OnInit } from '@angular/core';
import { BasketView, BasketService, DishView } from '../app-services/nswag.generated.services';

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

  constructor(
    private basketService: BasketService
  ) { }

  ngOnInit(): void {
    this.basketService.getAll().subscribe(data => {this.basket = data;
      this.dishes = data.dishes;
                                                  //  console.log(data);
                                                  //  console.log(this.basket);
                                                  });
  }

}
