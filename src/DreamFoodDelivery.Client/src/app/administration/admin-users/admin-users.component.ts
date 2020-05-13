import { Component, OnInit } from '@angular/core';
import { AdminService, UserView, UserDTO } from 'src/app/app-services/nswag.generated.services';

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
  ) { }

  ngOnInit(): void {
    this.adminService.getAll().subscribe(data => {this.users = data;
                                                  this.usersDTO = this.usersDTO;
                                                });
  }

  removeUser(id: string): void {
    this.adminService.removeById(id).subscribe(data => {
      const indexToDelete = this.usersDTO.findIndex((mark: UserDTO) => mark.id === id);
      this.users.splice(indexToDelete, 1);
    });
  }

  changeRole(idIdentity: string): void {
    this.adminService.changeRole(idIdentity).subscribe(data => {
      window.location.reload();
    });
  }

}
