import { Component, OnInit } from '@angular/core';
import { ReviewView, ReviewService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-comments',
  templateUrl: './user-comments.component.html',
  styleUrls: ['./user-comments.component.scss']
})
export class UserCommentsComponent implements OnInit {
  reviews: ReviewView[] = [];
  message: string = null;

  constructor(
    private reviewService: ReviewService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.reviewService.getByUserId().subscribe(data => {this.reviews = data;
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized';
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

  removeById(id: string): void {
    this.reviewService.removeById(id).subscribe(data => {
      const indexToDelete = this.reviews.findIndex((mark: ReviewView) => mark.id === id);
      this.reviews.splice(indexToDelete, 1);
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
}
