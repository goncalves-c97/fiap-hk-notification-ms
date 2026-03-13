variable "preferred_region" {
  description = "A região AWS preferida para a criação dos recursos."
  type        = string
  default     = "us-east-1"
}

variable "aws_profile" {
  description = "Optional named AWS CLI profile to use for credentials. Leave null to use default environment credentials."
  type        = string
  default     = null
}