import { Component, OnInit } from '@angular/core';
import { DishView, DishService, MenuService, TagToAdd } from 'src/app/app-services/nswag.generated.services';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { removeSpaces } from '../tag-validation';

@Component({
  selector: 'app-dish-update',
  templateUrl: './dish-update.component.html',
  styleUrls: ['./dish-update.component.scss']
})
export class DishUpdateComponent implements OnInit {
  idFromURL = '';
  dish: DishView;
  done = false;

  public dishInfoUpdateForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private dishService: DishService,
    private menuService: MenuService,
    private location: Location,
    public router: Router,
    public fb: FormBuilder,
  ) {
    route.params.subscribe(params => this.idFromURL = params.id);
    this.dishInfoUpdateForm = this.fb.group({
      id: [''],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      composition: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(250)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(250)]],
      cost: ['', [Validators.required]],
      weigh: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(250)]],
      sale: ['', [Validators.required]],
      tagNames: this.fb.array([
          this.initTag(), ])
    });
  }

  ngOnInit(): void {
    this.dishService.getById(this.idFromURL).subscribe(data => {this.dish = data;
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

  initTag() {
    return this.fb.group({
        tagName: ['', [Validators.required, removeSpaces]]
    });
  }

  addItem() {
    const control = this.dishInfoUpdateForm.controls.tagNames as FormArray;
    control.push(this.initTag());
  }

  removeItem(i: number) {
    const control = this.dishInfoUpdateForm.controls.tagNames as FormArray;
    control.removeAt(i);
  }

  updateDish(id: string): void {
    this.dishInfoUpdateForm.value.id = id;
    if (this.dishInfoUpdateForm.valid) {
      this.dishService.update(this.dishInfoUpdateForm.value).subscribe(data => {this.dish = data;
                                                                                this.done = true;
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
    } else {
      // TODO: message
    }
  }

  removeDish(id: string): void {
    this.dishService.removeById(id).subscribe(data => {
      this.router.navigate(['/dishes']);
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

  goBack(): void {
    this.location.back();
  }
}
