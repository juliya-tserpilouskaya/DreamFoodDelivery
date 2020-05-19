import { Component, OnInit } from '@angular/core';
import { CommentView, CommentService } from 'src/app/app-services/nswag.generated.services';
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
  comment: CommentView;
  commentAddForm: FormGroup;

  constructor(
    private commentService: CommentService,
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    public fb: FormBuilder,
  ) {
    route.params.subscribe(params => this.idFromURL = params.orderId);
    this.commentAddForm = this.fb.group({
      orderId: [''],
      headline: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      content: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(511)]],
      rating: [5, [Validators.required, Validators.min(0), Validators.max(5)]],
    });
   }

  ngOnInit(): void {
    console.log (this.idFromURL);
  }

  addNewComment(): void {
    if (this.commentAddForm.valid) {
      this.commentAddForm.value.orderId = this.idFromURL;
      this.commentService.create(this.commentAddForm.value)
        .subscribe(data => { this.comment = data;
                             this.router.navigate(['/comment', this.comment.id, 'details']);
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

  goBack(): void {
    this.location.back();
  }
}
