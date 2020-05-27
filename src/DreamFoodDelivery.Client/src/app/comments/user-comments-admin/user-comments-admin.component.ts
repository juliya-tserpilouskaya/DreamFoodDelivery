import { Component, OnInit } from '@angular/core';
import { CommentView, CommentService } from 'src/app/app-services/nswag.generated.services';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-user-comments-admin',
  templateUrl: './user-comments-admin.component.html',
  styleUrls: ['./user-comments-admin.component.scss']
})
export class UserCommentsAdminComponent implements OnInit {
  idFromURL = '';
  message: string = null;
  reviews: CommentView[] = [];

  constructor(
    private reviewService: CommentService,
    private route: ActivatedRoute,
    public router: Router,
    private location: Location,
  ) {
    route.params.subscribe(params => this.idFromURL = params.id);
  }

  ngOnInit(): void {
    this.reviewService.getByUserIdForAdmin(this.idFromURL).subscribe(data => {this.reviews = data;
    },
    error => {
      if (error.status ===  204) {
        this.message = 'Now empty.';
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
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
    // error => {console.log(error.status);
    //           console.log(error);
    //           console.log('got it!!!');
    });
  }

  goBack(): void {
    this.location.back();
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

  removeAll(): void {
    this.reviewService.removeAllByUserId(this.idFromURL).subscribe(result => {
      this.reviews = null;
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
