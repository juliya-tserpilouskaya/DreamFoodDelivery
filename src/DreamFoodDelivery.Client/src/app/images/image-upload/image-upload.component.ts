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
    });
  }

}
