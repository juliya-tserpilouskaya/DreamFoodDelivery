import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ManageMenuService } from 'src/app/app-services/manage-menu.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-internal-server-error',
  templateUrl: './internal-server-error.component.html',
  styleUrls: ['./internal-server-error.component.scss']
})
export class InternalServerErrorComponent implements OnInit {

  constructor(
    private location: Location,
    private manageMenuService: ManageMenuService,
    private route: ActivatedRoute,
    private router: Router,
  ) { }

  ngOnInit(): void {
  }

  goBack(): void {
    this.location.back();
  }

  get isAdmin(): boolean {
    return this.manageMenuService.isAdmin();
  }

  get errorMsg(): string {
    return this.route.snapshot.paramMap.get('msg');
  }

}
