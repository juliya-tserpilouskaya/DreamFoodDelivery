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
      weigh: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(250)]],
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
    } else {
      // TODO: message
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


