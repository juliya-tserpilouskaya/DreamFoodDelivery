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

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'menu', component: MenuSearchComponent},
  { path: 'profile', component: ProfileComponent  , canActivate: [AuthGuard] },
  { path: 'profile/update', component: ProfileUpdateComponent  , canActivate: [AuthGuard]},
  { path: 'basket', component: BasketComponent, canActivate: [AuthGuard]},
  { path: 'administration', component: AdministrationComponent, canActivate: [AuthGuard]},
  { path: 'about', component: AboutUsComponent},
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
