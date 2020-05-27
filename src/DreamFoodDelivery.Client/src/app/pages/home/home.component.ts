import { Component, OnInit } from '@angular/core';
import { CommentService, CommentForUsersView, PageResponseOfCommentForUsersView } from 'src/app/app-services/nswag.generated.services';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  comments: CommentForUsersView[] = [];
  response: PageResponseOfCommentForUsersView;
  requestForm: FormGroup;
  pages: number[] = [];

  constructor(
    private commentService: CommentService,
    private fb: FormBuilder,
  ) {
    this.requestForm = this.fb.group({
      pageNumber: 1,
      pageSize: 10
    });
  }

  ngOnInit(): void {
    this.getComments();
  }

  getComments(){
    this.commentService.getAll(this.requestForm.value).subscribe(response => {
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
