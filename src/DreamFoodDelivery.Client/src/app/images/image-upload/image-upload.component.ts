import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImageModifiedService } from 'src/app/app-services/image.services';

@Component({
  selector: 'app-image-upload',
  templateUrl: './image-upload.component.html',
  styleUrls: ['./image-upload.component.scss']
})
export class ImageUploadComponent implements OnInit {

  addressImage: string;
  message: string = null;
  selectedFile: File;


  constructor(
    private route: ActivatedRoute,
    private imageService: ImageModifiedService,
  ) {}

  ngOnInit(): void {
  }

  onFileChanged(event) {
    this.selectedFile = event.target.files[0];
    this.addressImage = null;
  }

  onUpload() {

    this.imageService.uploadImage(this.selectedFile, this.route.snapshot.paramMap.get('id'))
    .subscribe(data => {
      this.addressImage = data;
      this.selectedFile = null;
    },
    error => {
      if (error.status ===  400) {
        this.message = 'Error 400: ' + error.response;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  404) {
        this.message = 'Element not found.';
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
