import { Component, OnInit } from '@angular/core';
import { ReviewView, ReviewService } from 'src/app/app-services/nswag.generated.services';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-comment-create',
  templateUrl: './comment-create.component.html',
  styleUrls: ['./comment-create.component.scss']
})
export class CommentCreateComponent implements OnInit {
  idFromURL = '';
  review: ReviewView;
  reviewAddForm: FormGroup;
  message: string = null;

  constructor(
    private reviewService: ReviewService,
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    public fb: FormBuilder,
  ) {
    route.params.subscribe(params => this.idFromURL = params.orderId);
    this.reviewAddForm = this.fb.group({
      orderId: [''],
      headline: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      content: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(511)]],
      rating: [5, [Validators.required, Validators.min(0), Validators.max(5)]],
    });
   }

  ngOnInit(): void {
    // console.log (this.idFromURL);
  }

  addNewReview(): void {
    if (this.reviewAddForm.valid) {
      this.reviewAddForm.value.orderId = this.idFromURL;
      this.reviewService.create(this.reviewAddForm.value)
        .subscribe(data => { this.review = data;
                             this.router.navigate(['/review', data.id, 'details']);
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

  goBack(): void {
    this.location.back();
  }
}
