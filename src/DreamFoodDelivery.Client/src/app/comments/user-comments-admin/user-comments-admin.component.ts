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
  comments: CommentView[] = [];

  constructor(
    private commentService: CommentService,
    private route: ActivatedRoute,
    public router: Router,
    private location: Location,
  ) {
    route.params.subscribe(params => this.idFromURL = params.id);
  }

  ngOnInit(): void {
    this.commentService.getByUserIdForAdmin(this.idFromURL).subscribe(data => {this.comments = data;
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
    // error => {console.log(error.status);
    //           console.log(error);
    //           console.log('got it!!!');
    });
  }

  goBack(): void {
    this.location.back();
  }

  removeById(id: string): void {
    this.commentService.removeById(id).subscribe(data => {
      const indexToDelete = this.comments.findIndex((mark: CommentView) => mark.id === id);
      this.comments.splice(indexToDelete, 1);
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
  }

  removeAll(): void {
    this.commentService.removeAllByUserId(this.idFromURL).subscribe(result => {
      this.comments = null;
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
    // error => console.log(error));
  });
 }
}
