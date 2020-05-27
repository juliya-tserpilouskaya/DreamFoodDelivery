import { Component, OnInit } from '@angular/core';
import { CommentView, CommentService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-comments',
  templateUrl: './user-comments.component.html',
  styleUrls: ['./user-comments.component.scss']
})
export class UserCommentsComponent implements OnInit {
  reviews: CommentView[] = [];
  message: string = null;

  constructor(
    private reviewService: CommentService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.reviewService.getByUserId().subscribe(data => {this.reviews = data;
    },
    error => {
      if (error.status ===  204) {
        this.message = 'Now empty.';
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized';
      }
      else if (error.status ===  500) {
        this.message = 'Error 500: Internal Server Error!';
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }

  removeById(id: string): void {
    this.reviewService.removeById(id).subscribe(data => {
      const indexToDelete = this.reviews.findIndex((mark: CommentView) => mark.id === id);
      this.reviews.splice(indexToDelete, 1);
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  404) {
        this.message = 'Elements are not found.';
      }
      else if (error.status ===  500) {
        this.message = 'Error 500: Internal Server Error!';
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
    });
  }
}
