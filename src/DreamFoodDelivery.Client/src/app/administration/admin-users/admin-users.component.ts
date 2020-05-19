import { Component, OnInit } from '@angular/core';
import { AdminService, UserView, UserDTO } from 'src/app/app-services/nswag.generated.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.scss']
})
export class AdminUsersComponent implements OnInit {
  users: UserView[] = [];
  usersDTO: UserDTO[] = [];

  constructor(
    private adminService: AdminService,
    public router: Router,
  ) { }

  ngOnInit(): void {
    this.adminService.getAll().subscribe(data => {this.users = data;
                                                  this.usersDTO = this.usersDTO;
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

  removeUser(id: string): void {
    this.adminService.removeById(id).subscribe(data => {
      const indexToDelete = this.usersDTO.findIndex((mark: UserDTO) => mark.id === id);
      this.users.splice(indexToDelete, 1);
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

  changeRole(idIdentity: string): void {
    this.adminService.changeRole(idIdentity).subscribe(data => {
      window.location.reload();
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
