import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard } from './auth/auth.guard';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { BasketComponent } from './basket/basket.component';
import { MenuSearchComponent } from './menu-search/menu-search.component';
import { HomeComponent } from './pages/home/home.component';
import { ProfileComponent } from './profile-info/profile/profile.component';
import { AboutUsComponent } from './pages/about-us/about-us.component';
import { AdministrationComponent } from './administration/administration.component';
import { ProfileUpdateComponent } from './profile-info/profile-update/profile-update.component';
import { DishesComponent } from './dishes/dishes.component';
import { AdminUsersComponent } from './administration/admin-users/admin-users.component';
import { AdminCommentsComponent } from './administration/admin-comments/admin-comments.component';
import { AdminsOrdersComponent } from './administration/admins-orders/admins-orders.component';
import { ChangeUserDetailsComponent } from './administration/change-user-details/change-user-details.component';
import { CommentUpdateComponent } from './comments/comment-update/comment-update.component';
import { OrderUpdateComponent } from './orders/order-update/order-update.component';
import { EmployeeComponent } from './employee/employee.component';
import { EmployeeOrdersComponent } from './employee/employee-orders/employee-orders.component';
import { DishUpdateComponent } from './dishes/dish-update/dish-update.component';
import { DishAddNewComponent } from './dishes/dish-add-new/dish-add-new.component';
import { OrderCreateComponent } from './orders/order-create/order-create.component';
import { UserOrdersAdminComponent } from './orders/user-orders-admin/user-orders-admin.component';
import { CommentCreateComponent } from './comments/comment-create/comment-create.component';
import { UserCommentsAdminComponent } from './comments/user-comments-admin/user-comments-admin.component';
import { DeleteAccountComponent } from './auth/delete-account/delete-account.component';
import { ChangePasswordComponent } from './auth/change-password/change-password.component';
import { ChangeEmailComponent } from './auth/change-email/change-email.component';
import { ConfirmEmailComponent } from './auth/confirm-email/confirm-email.component';
import { BadRequestComponent } from './pages/bad-request/bad-request.component';
import { InternalServerErrorComponent } from './pages/internal-server-error/internal-server-error.component';
import { UnexpectedComponent } from './pages/unexpected/unexpected.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'delete', component: DeleteAccountComponent },
  { path: 'email/change', component: ChangeEmailComponent },
  { path: 'email/confirm', component: ConfirmEmailComponent },
  { path: 'password/change', component: ChangePasswordComponent },

  { path: 'menu', component: MenuSearchComponent},
  { path: 'basket', component: BasketComponent, canActivate: [AuthGuard]},

  { path: 'profile', component: ProfileComponent  , canActivate: [AuthGuard] },
  { path: 'profile/update', component: ProfileUpdateComponent  , canActivate: [AuthGuard]},

  { path: 'administration', component: AdministrationComponent, canActivate: [AuthGuard]},
  { path: 'administration/users', component: AdminUsersComponent, canActivate: [AuthGuard]},
  { path: 'administration/user/:id', component: ChangeUserDetailsComponent, canActivate: [AuthGuard]},

  { path: 'administration/comments', component: AdminCommentsComponent, canActivate: [AuthGuard]},
  { path: 'administration/comments/user/:id', component: UserCommentsAdminComponent, canActivate: [AuthGuard]},

  { path: 'administration/orders', component: AdminsOrdersComponent, canActivate: [AuthGuard]},
  { path: 'administration/orders/user/:id', component: UserOrdersAdminComponent, canActivate: [AuthGuard]},

  { path: 'orders/status', component: EmployeeComponent, canActivate: [AuthGuard]},
  { path: 'orders/status/:statusName', component: EmployeeOrdersComponent, canActivate: [AuthGuard]},

  { path: 'order/create', component: OrderCreateComponent, canActivate: [AuthGuard]},
  { path: 'order/:id/details', component: OrderUpdateComponent, canActivate: [AuthGuard]},

  { path: 'comment/create/for/:orderId', component: CommentCreateComponent, canActivate: [AuthGuard]},
  { path: 'comment/:id/details', component: CommentUpdateComponent, canActivate: [AuthGuard]},

  { path: 'dishes', component: DishesComponent, canActivate: [AuthGuard]},
  { path: 'dish/:id/details', component: DishUpdateComponent, canActivate: [AuthGuard]},
  { path: 'dish/create', component: DishAddNewComponent, canActivate: [AuthGuard]},

  { path: 'about', component: AboutUsComponent},
  { path: 'error/500', component: InternalServerErrorComponent},
  { path: 'error/400', component: BadRequestComponent},
  { path: 'error/unexpected', component: UnexpectedComponent},
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
