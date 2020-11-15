import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  public member:Member;
  public galleryOptions:NgxGalleryOptions[];
  public galleryImages:NgxGalleryImage[];

  constructor(private memberService: MemberService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();

    this.galleryOptions = [{
      width: '500px',
      height: '500px',
      imagePercent: 100,
      thumbnailsColumns: 4,
      preview: false,
      imageAnimation: NgxGalleryAnimation.Slide
    }];

    
  }

  getImages(){
    const imageUrls = [];
    for(const photo of this.member.photos){
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      });

    }
    return imageUrls;
  }

  loadMember(){
    let userName:string = this.route.snapshot.paramMap.get('username');
    console.log(userName);
    this.memberService.getMember(userName)
      .subscribe(m => {
        this.member = m;
        this.galleryImages = this.getImages();
      });
  }

}
