import { Component, OnInit } from '@angular/core';
import { CommentService, CommentView } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';


@Component({
  selector: 'app-admin-comments',
  templateUrl: './admin-comments.component.html',
  styleUrls: ['./admin-comments.component.scss']
})
export class AdminCommentsComponent implements OnInit {
  reviews: CommentView[] = [];
  requestForm: FormGroup;
  pages: number[] = [];

  page = 2;
  pageSize = 10;

  constructor(
    private reviewsService: CommentService,
    public router: Router,
  ) {
   }

  ngOnInit(): void {
    this.reviewsService.getAllAdmin().subscribe(data => this.reviews = data);

  }

  removeById(id: string): void {
    this.reviewsService.removeById(id).subscribe(data => {
      const indexToDelete = this.reviews.findIndex((mark: CommentView) => mark.id === id);
      this.reviews.splice(indexToDelete, 1);
    },
    error => {

    });
  }

  removeAll(): void {
    this.reviewsService.removeAll().subscribe(data => {
      this.ngOnInit();

    },
    error => {

    });
  }
}
