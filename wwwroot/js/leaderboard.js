class LeaderboardManager {
    constructor() {
        this.initializeEventListeners();
    }

    initializeEventListeners() {
        // Atualizar resultado quando o input mudar
        $(document).on('change', '.result-input', (e) => {
            const input = $(e.target);
            const teamId = input.data('team-id');
            const workoutId = input.data('workout-id');
            const result = parseFloat(input.val()) || 0;

            this.updateResult(teamId, workoutId, result);
        });

        // Reset do leaderboard
        $('#resetBtn').on('click', () => {
            if (confirm('Tem certeza que deseja resetar o leaderboard?')) {
                this.resetLeaderboard();
            }
        });
    }

    updateResult(teamId, workoutId, result) {
        $.ajax({
            url: '/Home/UpdateResult',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                teamId: teamId,
                workoutId: workoutId,
                result: result
            }),
            success: (response) => {
                if (response.success) {
                    this.updateLeaderboardDisplay(response.leaderboard);
                } else {
                    alert('Erro ao atualizar resultado: ' + response.message);
                }
            },
            error: () => {
                alert('Erro ao comunicar com o servidor');
            }
        });
    }

    resetLeaderboard() {
        $.ajax({
            url: '/Home/ResetLeaderboard',
            type: 'POST',
            success: (response) => {
                if (response.success) {
                    this.updateLeaderboardDisplay(response.leaderboard);
                    $('.result-input').val('');
                } else {
                    alert('Erro ao resetar leaderboard');
                }
            },
            error: () => {
                alert('Erro ao comunicar com o servidor');
            }
        });
    }

    updateLeaderboardDisplay(leaderboard) {
        // Atualizar valores dos inputs
        leaderboard.teams.forEach((team) => {
            leaderboard.workouts.forEach((workout) => {
                const result = leaderboard.results.find(r => 
                    r.teamId === team.id && r.workoutId === workout.id);
                
                const input = $(`.result-input[data-team-id="${team.id}"][data-workout-id="${workout.id}"]`);
                input.val(result.result > 0 ? result.result : '');

                // Atualizar badge de posição
                this.updatePositionBadge(input, result);
            });
        });

        // Atualizar pontos totais e reordenar
        this.updateTotalPoints(leaderboard.teams);
        this.reorderTeams();
    }

    updatePositionBadge(input, result) {
        const cell = input.closest('td');
        const positionBadge = cell.find('.position-badge');
        
        if (result.position > 0) {
            if (positionBadge.length === 0) {
                cell.append(`<div class="position-badge"><span class="badge ${this.getPositionBadgeClass(result.position)}">${this.getPositionText(result.position)}</span></div>`);
            } else {
                positionBadge.find('.badge')
                    .removeClass()
                    .addClass(`badge ${this.getPositionBadgeClass(result.position)}`)
                    .text(this.getPositionText(result.position));
            }
        } else {
            positionBadge.remove();
        }
    }

    updateTotalPoints(teams) {
        teams.forEach((team) => {
            const totalCell = $(`tr:has(.result-input[data-team-id="${team.id}"]) td:last`);
            totalCell.text(team.totalPoints);
            totalCell.removeClass('bg-success text-white');
            if (team.totalPoints > 0) {
                totalCell.addClass('bg-success text-white');
            }
        });
    }

    reorderTeams() {
        const tbody = $('#leaderboardTable tbody');
        const rows = tbody.find('tr').get();
        rows.sort((a, b) => {
            const aPoints = parseInt($(a).find('td:last').text()) || 0;
            const bPoints = parseInt($(b).find('td:last').text()) || 0;
            return aPoints - bPoints;
        });
        tbody.empty().append(rows);
    }

    getPositionBadgeClass(position) {
        switch(position) {
            case 1: return 'bg-warning text-dark';
            case 2: return 'bg-secondary';
            case 3: return 'bg-warning text-dark';
            default: return 'bg-secondary';
        }
    }

    getPositionText(position) {
        switch(position) {
            case 1: return '🥇 1º';
            case 2: return '🥈 2º';
            case 3: return '🥉 3º';
            case 4: return '4º';
            case 5: return '5º';
            default: return '';
        }
    }
}

// Inicializar quando o documento estiver pronto
$(document).ready(() => {
    new LeaderboardManager();
}); 