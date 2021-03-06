import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ReviewView, ReviewService } from 'src/app/app-services/nswag.generated.services';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/auth.service';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-comment-update',
  templateUrl: './comment-update.component.html',
  styleUrls: ['./comment-update.component.scss']
})
export class CommentUpdateComponent implements OnInit {
  idFromURL = '';
  message: string = null;
  review: ReviewView;
  reviewUpdateForm: FormGroup;
  done = false;

  constructor(
    private reviewService: ReviewService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private location: Location,
    public router: Router,
    public fb: FormBuilder,
    ) {
    route.params.subscribe(params => this.idFromURL = params.id);
    this.reviewUpdateForm = this.fb.group({
      id: [''],
      headline: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      content: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(511)]],
      rating: ['', [Validators.required, Validators.min(0), Validators.max(5)]],
    });
  }

  ngOnInit(): void {
    this.reviewService.getById(this.idFromURL).subscribe(data => {this.review = data;
    },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
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

  reviewUpdate(id: string): void {
    this.reviewUpdateForm.value.id = id;
    if (this.reviewUpdateForm.valid) {
      this.reviewService.update(this.reviewUpdateForm.value)
      .subscribe(data => {this.review = data;
                          this.done = true;
                          },
                          error => {
                          if (error.status ===  400) {
                            this.message = 'Error 400: ' + error.result403;
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

  removeById(id: string): void {
    this.reviewService.removeById(id).subscribe(data => {},
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
        else if (error.status ===  500) {
          this.message = 'Error 500: Internal Server Error!';
        }
        else{
          this.message = 'Something was wrong. Please, contact with us.';
        }
      });
  }


  goBack(): void {
    this.location.back();
  }

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    // debugger;
    const decodedToken = jwt_decode(token);
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedToken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }

}
