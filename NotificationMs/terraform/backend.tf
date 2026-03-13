terraform {
  backend "s3" {
    bucket = "fiap-terraform-backend-infra-tf"
    key    = "hk/notification-ms/terraform.tfstate"
    region = "us-east-1"
  }
}
