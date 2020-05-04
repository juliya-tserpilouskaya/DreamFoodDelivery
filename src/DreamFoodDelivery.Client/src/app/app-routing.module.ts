import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ProfileComponent } from './profile/profile.component';
import { LoginComponent } from './auth/login/login.component';

import { AuthGuardGuard } from "./auth/auth-guard.guard";
import { RegisterComponent } from './auth/register/register.component';
import { DishMenuComponent } from './dish-menu/dish-menu.component';
import { SampleComponent } from './sample/sample.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { BasketComponent } from './basket/basket.component';



const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'profile', component: ProfileComponent /*, canActivate: [AuthGuardGuard]*/},
  { path: 'menu', component: DishMenuComponent},
  { path: 'basket', component: BasketComponent},

  { path: '', component: SampleComponent, pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
