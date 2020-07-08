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
  message: string = null;
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
      });
  }

  removeAll(): void {
    this.manageMenuService.removeAll().then(data => this.dishes = data)
    .catch(msg => {
      if (msg.status ===  206) {
        this.message = msg.detail;
      }
      else if (msg.status ===  400) {
        this.message = 'Error 400: ' + msg.result400;
      }
      else if (msg.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (msg.status ===  500) {
        this.message = msg.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }

  removeDish(id: string): void {
    this.dishService.removeById(id).subscribe(data => {
      const indexToDelete = this.dishes.findIndex((mark: DishView) => mark.id === id);
      this.dishes.splice(indexToDelete, 1);
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
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
