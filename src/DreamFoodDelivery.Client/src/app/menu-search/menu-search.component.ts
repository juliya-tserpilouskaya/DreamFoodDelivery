import { Component, OnInit } from '@angular/core';

// import service and models
import { MenuService, DishView } from '../app-services/nswag.generated.services';

@Component({
  selector: 'app-menu-search',
  templateUrl: './menu-search.component.html',
  styleUrls: ['./menu-search.component.scss']
})
export class MenuSearchComponent implements OnInit {

  dishes: DishView[] = [];

  constructor(
    private menuService: MenuService,
  ) { }

  ngOnInit(): void {
    this.menuService.getAll().subscribe(data => this.dishes = data);
  }
}
