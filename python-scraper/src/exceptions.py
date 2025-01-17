class EnvironmentVariableError(Exception):
    """Custom exception for missing or invalid environment variables."""
    def __init__(self, variable_name):
        super().__init__(f"Environment variable {variable_name} is missing or invalid.")
        self.variable_name = variable_name

