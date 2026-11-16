
using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Enums;
using ValiaApiWeb.Domain.Models;

namespace ValiaApiWeb.Data
{
    public static class WorkConditionQuestionSeeder
    {
        private const int QuestionnaireVersion = 1;

        public static async Task SeedAsync(ValiaDbContext context)
        {
            bool versionExists = await context.WorkConditionQuestions
                .AnyAsync(q => q.QuestionnaireVersion == QuestionnaireVersion);

            if (versionExists)
                return;

            var questions = new List<WorkConditionQuestion>
            {
                new("Você consegue realizar pausas adequadas durante sua jornada de trabalho?", EnumWorkConditionCategory.JornadaEDescanso, 1, QuestionnaireVersion),
                new("Você costuma ter tempo suficiente de descanso entre uma jornada de trabalho e outra?", EnumWorkConditionCategory.JornadaEDescanso, 2, QuestionnaireVersion),

                new("Seu pagamento costuma ser realizado dentro do prazo esperado?", EnumWorkConditionCategory.Remuneracao, 3, QuestionnaireVersion),
                new("Os valores que você recebe correspondem ao que foi combinado para seu trabalho?", EnumWorkConditionCategory.Remuneracao, 4, QuestionnaireVersion),

                new("Você sente que é tratado(a) com respeito no ambiente de trabalho?", EnumWorkConditionCategory.RespeitoProfissional, 5, QuestionnaireVersion),
                new("Você se sente à vontade para expressar dúvidas ou preocupações profissionais?", EnumWorkConditionCategory.RespeitoProfissional, 6, QuestionnaireVersion),

                new("Você considera que seu ambiente de trabalho oferece condições adequadas de segurança?", EnumWorkConditionCategory.Seguranca, 7, QuestionnaireVersion),
                new("Você recebe orientações e recursos adequados para realizar suas atividades com segurança?", EnumWorkConditionCategory.Seguranca, 8, QuestionnaireVersion),

                new("Você recebe informações sobre seus horários e escalas com antecedência suficiente?", EnumWorkConditionCategory.Previsibilidade, 9, QuestionnaireVersion),
                new("Mudanças nas suas atividades ou horários costumam ser comunicadas de maneira clara?", EnumWorkConditionCategory.Previsibilidade, 10, QuestionnaireVersion),

                new("Você dispõe dos recursos necessários para realizar suas atividades?", EnumWorkConditionCategory.CondicoesGerais, 11, QuestionnaireVersion),
                new("De maneira geral, você considera satisfatórias suas condições de trabalho?", EnumWorkConditionCategory.CondicoesGerais, 12, QuestionnaireVersion)
            };

            await context.WorkConditionQuestions.AddRangeAsync(questions);
            await context.SaveChangesAsync();
        }
    }
}

