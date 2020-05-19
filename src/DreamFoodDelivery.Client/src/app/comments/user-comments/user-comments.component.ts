import { Component, OnInit } from '@angular/core';
import { CommentView, CommentService } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-comments',
  templateUrl: './user-comments.component.html',
  styleUrls: ['./user-comments.component.scss']
})
export class UserCommentsComponent implements OnInit {
  comments: CommentView[] = [];

  constructor(
    private commentService: CommentService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.commentService.getByUserId().subscribe(data => {this.comments = data;
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
}
