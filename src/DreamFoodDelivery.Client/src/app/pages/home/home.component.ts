import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ThrowStmt } from '@angular/compiler';
import { RatingView, ReviewForUsersView, PageResponseOfReviewForUsersView, ReviewService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  currentRate: number;
  comments: ReviewForUsersView[] = [];
  response: PageResponseOfReviewForUsersView;
  requestForm: FormGroup;
  pages: number[] = [];
  rating: RatingView;
  message: string = null;

  constructor(
    private reviewService: ReviewService,
    private fb: FormBuilder,
    public router: Router
  ) {
    this.requestForm = this.fb.group({
      pageNumber: 1,
      pageSize: 10
    });
  }

  ngOnInit(): void {
    this.getComments();
    this.getRating();
  }

  getRating(){
    this.reviewService.getRating().subscribe(data => {this.rating = data;
                                                      this.currentRate = data.sum;
                                                      this.message = null;
    },
     error => {
      if (error.status ===  206) {
        this.message = error.detail;
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

  getComments(){
    this.reviewService.getAll(this.requestForm.value).subscribe(response => {
      this.response = response;
      console.log(response);
      this.pages = [];
      for (let index = 1; index <= response.totalPages; index++) {
        this.pages.push(index);
      }
    },
    error => {
      this.response = undefined;

    });
  }

  previousePage(){
    if (this.response != null && this.response.hasPreviousPage) {
      this.requestForm.value.pageNumber -= 1;
      this.ngOnInit();
    }
  }

  nextPage(){
    if (this.response != null && this.response.hasNextPage) {
      this.requestForm.value.pageNumber += 1;
      this.ngOnInit();
    }
  }

  getPage(page: number){
    if (this.response != null && page > 0 && page <= this.response.totalPages) {
      this.requestForm.value.pageNumber = page;
      this.ngOnInit();
    }
  }
}

