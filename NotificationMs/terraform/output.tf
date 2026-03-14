output "ecs_cluster_name" {
  description = "Cluster ECS compartilhado."
  value       = data.terraform_remote_state.infra.outputs.ecs_cluster_name
}

output "ecs_service_name" {
  description = "Nome do servico ECS de notificacao."
  value       = data.terraform_remote_state.infra.outputs.ecs_service_names.notification
}

output "shared_secret_arn" {
  description = "ARN do secret compartilhado com RabbitMQ e SMTP."
  value       = data.terraform_remote_state.infra.outputs.shared_secret_arn
}

output "rabbitmq_host" {
  description = "Hostname interno do RabbitMQ."
  value       = data.terraform_remote_state.infra.outputs.rabbitmq_host
}

output "container_image" {
  description = "Imagem Docker Hub configurada para o notification-ms."
  value       = data.terraform_remote_state.infra.outputs.dockerhub_images.notification
}
