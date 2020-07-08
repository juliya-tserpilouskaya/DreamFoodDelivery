import { Component, OnInit } from '@angular/core';
import { ReviewService, ReviewView } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';


@Component({
  selector: 'app-admin-comments',
  templateUrl: './admin-comments.component.html',
  styleUrls: ['./admin-comments.component.scss']
})
export class AdminCommentsComponent implements OnInit {
  reviews: ReviewView[] = [];
  requestForm: FormGroup;
  pages: number[] = [];
  message: string = null;

  page = 2;
  pageSize = 10;

  constructor(
    private reviewsService: ReviewService,
    public router: Router,
  ) {
   }

  ngOnInit(): void {
    this.reviewsService.getAllAdmin().subscribe(data => { this.reviews = data; },
      error => {
        if (error.status ===  206) {
          this.message = error.detail;
          this.reviews = [];
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
      }
    );
  }

  removeById(id: string): void {
    this.reviewsService.removeById(id).subscribe(data => { this.ngOnInit();
      // const indexToDelete = this.reviews.findIndex((mark: ReviewView) => mark.id === id);
      // this.reviews.splice(indexToDelete, 1);
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  404) {
        this.message = 'Element not found.';
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

  removeAll(): void {
    this.reviewsService.removeAll().subscribe(data => {
      this.ngOnInit();
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
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
}
