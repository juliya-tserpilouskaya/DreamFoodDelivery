import { Component, OnInit } from '@angular/core';

// import service
import { MenuService } from 'src/app/nswag_gen/services/api.generated.client';
// import models
import { DishView } from 'src/app/nswag_gen/services/api.generated.client';

@Component({
  selector: 'app-dish-menu',
  templateUrl: './dish-menu.component.html',
  styleUrls: ['./dish-menu.component.scss']
})
export class DishMenuComponent implements OnInit {

  dishes: DishView[] = [];

  constructor(
    private menuService: MenuService,
  ) { }

  ngOnInit(): void {
    this.menuService.getAll().subscribe(data => this.dishes = data);
  }
}
