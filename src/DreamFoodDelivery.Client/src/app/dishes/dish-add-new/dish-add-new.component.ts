import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { DishService, DishView } from 'src/app/app-services/nswag.generated.services';
import { Location } from '@angular/common';
import { removeSpaces } from '../tag-validation';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dish-add-new',
  templateUrl: './dish-add-new.component.html',
  styleUrls: ['./dish-add-new.component.scss']
})
export class DishAddNewComponent implements OnInit {

  dish: DishView;
  dishAddForm: FormGroup;
  message: string = null;

  constructor(
    private dishService: DishService,
    private location: Location,
    private router: Router,
    public fb: FormBuilder,
  ) {
    this.dishAddForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(90)]],
      composition: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(250)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(250)]],
      cost: ['', [Validators.required]],
      weight: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(250)]],
      sale: ['', [Validators.required]],
      tagNames: this.fb.array([
          this.initTag(), ])
    });
  }

  ngOnInit(): void {
  }

  addNewDish(): void {
    if (this.dishAddForm.valid) {
      // console.log(this.dishAddForm.value);
      this.dishService.create(this.dishAddForm.value)
        .subscribe(data => { this.dish = data;
                             this.router.navigate(['/dish', this.dish.id, 'details']);
                            },
                            error => {
                              if (error.status ===  400) {
                                this.message = 'Error 400: ' + error.response;
                              }
                              else if (error.status ===  403) {
                                this.message = 'You are not authorized!';
                              }
                              else if (error.status ===  500) {
                                this.message = 'Error 500: Internal Server Error!';
                              }
                              else{
                                this.message = 'Something was wrong. Please, contact with us.';
                              }
                              });
    }
  }

  initTag() {
    return this.fb.group({
      tagName: ['', [Validators.required, removeSpaces]]
    });
  }

  addItem() {
    const control = this.dishAddForm.controls.tagNames as FormArray;
    control.push(this.initTag());
  }

  removeItem(i: number) {
    const control = this.dishAddForm.controls.Items as FormArray;
    control.removeAt(i);
  }

  goBack(): void {
    this.location.back();
  }
}


