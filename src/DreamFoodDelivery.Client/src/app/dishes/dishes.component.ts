import { Component, OnInit } from '@angular/core';
import { DishView, DishService } from '../app-services/nswag.generated.services';
import { ManageMenuService } from '../app-services/manage-menu.service';
import { Router } from '@angular/router';
import { ImageModifiedService } from '../app-services/image.services';

@Component({
  selector: 'app-dishes',
  templateUrl: './dishes.component.html',
  styleUrls: ['./dishes.component.scss']
})
export class DishesComponent implements OnInit {
  dishes: DishView[] = [];
  mainImages: {[id: string]: string} = { };

  constructor(
    private manageMenuService: ManageMenuService,
    private imageService: ImageModifiedService,
    private dishService: DishService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.manageMenuService.getAllDishes()
      .then(data => {this.dishes = data;
                     if (this.dishes) {
                    for (const dish of this.dishes){
                      this.getImages(dish.id);
          }
         }

      })
      .catch(msg => console.log(msg));
  }

  removeAll(): void {
    this.manageMenuService.removeAll().then(data => this.dishes = data)
    .catch(msg => {console.log(msg); });
  }

  removeDish(id: string): void {
    this.dishService.removeById(id).subscribe(data => {
      const indexToDelete = this.dishes.findIndex((mark: DishView) => mark.id === id);
      this.dishes.splice(indexToDelete, 1);
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
}
