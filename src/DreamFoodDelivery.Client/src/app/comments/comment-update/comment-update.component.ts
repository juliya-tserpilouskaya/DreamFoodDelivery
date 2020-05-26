import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { CommentView, CommentService } from 'src/app/app-services/nswag.generated.services';
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
  review: CommentView;
  reviewUpdateForm: FormGroup;
  done = false;

  constructor(
    private reviewService: CommentService,
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
      if (error.status === 500){
        this.router.navigate(['/error/500']);
       }
       else if (error.status === 404) {
        this.router.navigate(['/error/404']);
       }
       else {
        this.router.navigate(['/error/unexpected']);
       }
      });
  }

  reviewUpdate(id: string): void {
    this.reviewUpdateForm.value.id = id;
    if (this.reviewUpdateForm.valid) {
      this.reviewService.update(this.reviewUpdateForm.value).subscribe(data => {this.review = data;
                                                                                this.done = true;
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
    } else {
      // TODO: message
    }
  }

  removeById(id: string): void {
    this.reviewService.removeById(id).subscribe(data => {},
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


  goBack(): void {
    this.location.back();
  }

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedToken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }

}
