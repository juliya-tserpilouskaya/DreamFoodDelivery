import { Component, OnInit } from '@angular/core';
import { CommentService, CommentView, PageResponseOfCommentView } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';


@Component({
  selector: 'app-admin-comments',
  templateUrl: './admin-comments.component.html',
  styleUrls: ['./admin-comments.component.scss']
})
export class AdminCommentsComponent implements OnInit {
  comments: CommentView[] = [];
  response: PageResponseOfCommentView;
  requestForm: FormGroup;
  pages: number[] = [];

  constructor(
    private commentService: CommentService,
    private fb: FormBuilder,
    public router: Router,
  ) {
    this.requestForm = this.fb.group({
      pageNumber: 1,
      pageSize: 3
    });
   }

  ngOnInit(): void {

  }

  getComments(){
    console.log(this.requestForm.value);
    this.commentService.getAll(this.requestForm.value).subscribe(response => {
      this.response = response;
      this.pages = [];
      for (let index = 1; index <= response.totalPages; index++) {
        this.pages.push(index);
      }
    },
    error => {
      // if (error.status === 500){
      //   this.router.navigate(['/error/500']);
      //  }
      //  else if (error.status === 404) {
      //   this.router.navigate(['/error/404']);
      //  }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
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

  removeById(id: string): void {
    this.commentService.removeById(id).subscribe(data => {
      const indexToDelete = this.comments.findIndex((mark: CommentView) => mark.id === id);
      this.comments.splice(indexToDelete, 1);
    },
    error => {
      // if (error.status === 500){
      //   this.router.navigate(['/error/500']);
      //  }
      //  else if (error.status === 404) {
      //   this.router.navigate(['/error/404']);
      //  }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
    });
  }

  removeAll(): void {
    this.commentService.removeAll().subscribe(data => {window.location.reload();
    },
    error => {
      // if (error.status === 500){
      //   this.router.navigate(['/error/500']);
      //  }
      //  else if (error.status === 404) {
      //   this.router.navigate(['/error/404']);
      //  }
      //  else {
      //   this.router.navigate(['/error/unexpected']);
      //  }
    });
  }
}
