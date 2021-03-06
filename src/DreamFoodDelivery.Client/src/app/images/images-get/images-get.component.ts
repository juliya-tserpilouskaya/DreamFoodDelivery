import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ImageModifiedService } from 'src/app/app-services/image.services';
import { AuthService } from 'src/app/auth/auth.service';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-images-get',
  templateUrl: './images-get.component.html',
  styleUrls: ['./images-get.component.scss']
})
export class ImagesGetComponent implements OnInit {

  imagesCount: number[] = [];
  currentImageNumber = 1;
  message: string = null;
  allImages: string[];
  currentImage: string;

  currentDishId = this.route.snapshot.paramMap.get('id');

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private imageService: ImageModifiedService,
    private authService: AuthService

  ) {  }

  ngOnInit(): void {
    this.getImages();
  }

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    const decodedToken = jwt_decode(token);
    // tslint:disable-next-line: no-string-literal
    const currentRole = decodedToken['role'];
    if (currentRole.includes('Admin')) {
      return true;
    }
    return false;
  }

  getImages(){
      this.imageService.getImageNamesList(this.currentDishId)
    .subscribe(allImages => {
      this.allImages = allImages;
      this.imagesCount = [];
      if (this.allImages) {
        for (let index = 1; index <= this.allImages.length; index++) {
          this.imagesCount.push(index);
        }
        this.getImage(this.currentImageNumber); }
      },
    error => {
      if (error.status ===  206) {
        this.message = error.detail;
      }
      else if (error.status ===  400) {
        this.message = 'Error 400: ' + error.result400;
      }
      else if (error.status ===  403) {
        this.message = 'You are not authorized!';
      }
      else if (error.status ===  500) {
        this.message = error.message;
        this.router.navigate(['/error/500', {msg: this.message}]);
      }
      else{
        this.message = 'Something was wrong. Please, contact with us.';
      }
     }
    );
  }

  previouseImage(){
    if (this.allImages != null && this.currentImageNumber > 1) {
      this.currentImageNumber -= 1;
      this.getImage(this.currentImageNumber);
    }
  }

  nextImage(){
    if (this.allImages != null && this.currentImageNumber < this.allImages.length) {
      this.currentImageNumber += 1;
      this.getImage(this.currentImageNumber);
    }
  }

  getImage(imageNumber: number){
    if (this.allImages != null && imageNumber > 0 && imageNumber <= this.allImages.length) {
      this.currentImageNumber = imageNumber;
      this.imageService.getImage(this.currentDishId, this.allImages[imageNumber - 1])
      .subscribe(data => this.currentImage = data);
    }
  }

  show(){
    this.getImages();
  }

  deleteImage(){
    if (this.allImages != null && this.currentImageNumber > 0 && this.currentImageNumber <= this.allImages.length) {
      this.imageService.delete(this.currentDishId, this.allImages[this.currentImageNumber - 1])
      .subscribe(() => {
        this.currentImage = null;
        this.currentImageNumber = 1;
        this.getImages();
      });
    }
  }

}
