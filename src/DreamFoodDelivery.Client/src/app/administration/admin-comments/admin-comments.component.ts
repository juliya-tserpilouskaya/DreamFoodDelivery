import { Component, OnInit } from '@angular/core';
import { CommentService, CommentView } from 'src/app/app-services/nswag.generated.services';


@Component({
  selector: 'app-admin-comments',
  templateUrl: './admin-comments.component.html',
  styleUrls: ['./admin-comments.component.scss']
})
export class AdminCommentsComponent implements OnInit {
  comments: CommentView[] = [];

  constructor(
    private commentService: CommentService,
  ) { }

  ngOnInit(): void {
    this.commentService.getAll().subscribe(data => {this.comments = data;
    });

  }

  removeById(id: string): void {
    this.commentService.removeById(id).subscribe(data => {
      const indexToDelete = this.comments.findIndex((mark: CommentView) => mark.id === id);
      this.comments.splice(indexToDelete, 1);
    });
  }

  removeAll(): void {
    this.commentService.removeAll().subscribe(data => {window.location.reload();
    });
  }
}
